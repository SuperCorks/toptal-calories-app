/*
 * This code follow this method to check the token :
 * https://auth0.com/docs/api-auth/tutorials/verify-access-token
 */
const uuid = require('uuid/v4');
const request = require('request');
const jwt = require('jsonwebtoken');
const jwksClient = require('jwks-rsa');


const User = require('calories/users/entities/user');
const SessionDao = require('../daos/session-info-dao');
const SessionInfo = require('../entities/session-info');
const UserDao = require('calories/users/daos/user-dao');

const userDao = new UserDao();
const sessionDao = new SessionDao();

/**
 * To retrieve the RSA key from a JWKS used in the github example of json web token
 * (at the end of the exemple, there also another way with a .pem file )
 * @see https://github.com/auth0/node-jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback
 */


/** @type {OAuthAppConfigs} */
const CurrentOAuthConfigs = {
    signingAlgorithms: ['RS256'],
    audience: 'https://supercorks.auth0.com/api/v2/',
    issuer: "https://supercorks.auth0.com/",
    userInfoUrl: 'https://supercorks.auth0.com/userinfo',
    userSettingsUrl:  'https://supercorks.auth0.com/api/v2/',
    keyId: "OTQ4OURCRTIxNzQ1MDBCNTY2MTBFNUYzRUNGQzRFQTVBQUJDOTMxRg",
    publicKeyUrl: 'https://supercorks.auth0.com/.well-known/jwks.json',
};

/**
 * @description Lifetime (in millisecond) of a session.
 * @type {number}
 */
const SessionTimeToLive = 30 * 60 * 1000; // 30 minutes

/**
 * @description Used to indicate the maximum number of retry to create a unique UUID that does not exist withing the
 * activeSession array before stopping and throwing an error.
 */
const MaxUuidRetries = 4;

const AuthTokenHeader = 'auth-token';


/**
 * @description Manages authentication (login, signup, sessions, etc.).
 */
class AuthenticationManager {

    /**
     * @param {OAuthAppConfigs} oauthAppConfigs - OAuth configs used by this manager.
     */
    constructor(oauthAppConfigs) {

        this.testUser = undefined;

        /**
         * @description Whether this manager is done initializing.
         */
        this.isInitialized = false;

        /**
         * @private
         *
         * @description Cached public key used to verify messages from Auth0 server.
         *
         * @type {string|undefined}
         */
        this.oAuthAppPublicKey = undefined;

        /**
         * @private
         *
         * @description OAuth configs used by this manager.
         *
         * @type {OAuthAppConfigs}
         */
        this.oauthAppConfigs = oauthAppConfigs;
    }

    /**
     * @public
     *
     * @description Initializes this manager. This must be called only once and called before any other public method in
     * this class.
     *
     * @return {Promise<void>} A promise that resolves when initialization is completed.
     */
    async initialize() {

        if (this.isInitialized) throw new Error(`Cannot initialize an ${AuthenticationManager.name} twice!`);

        this.oAuthAppPublicKey = await this.getSigningKeyFromOAuthServer();

        this.isInitialized = true;
    }


    /**
     * @public
     *
     * @description Logs in (i.e. starts a new session) a user identified by the provided access token.
     *
     * @param {string} accessToken - The OAuth2 access token with the 'session:create' permission that identifies the
     * user to log in. Must be encoded.
     *
     * @returns {Promise<SessionInfo>} A promise that resolves when the login process finishes and returns a session
     * object for the created session.
     *
     * @throws {Error} If the manager is not initialized.
     */
    async login(accessToken) {
        this.throwIfNotInitialized();

        const decodedToken = await this.verifyAccessToken(accessToken);

        const userInfo = await this.fetchUserInfo(accessToken); // will throw if no email in user info.

        const correspondingUser = await userDao.get(userInfo.email);

        if (!correspondingUser)
            return await this.signup(accessToken, true);

        return await this.createSession(accessToken, correspondingUser);
    }

    /**
     * @public
     *
     * @description Logs in (i.e. starts a new session) a user identified by the provided username (no password).
     *
     * @param {string} username - The username of the user to login.
     *
     * @returns {Promise<SessionInfo>} A promise that resolves when the login process finishes and returns a session
     * object for the created session.
     *
     * @throws {Error} If the manager is not initialized.
     */
    async loginUnsafe(username) {
        this.throwIfNotInitialized();

        const correspondingUser = await userDao.get(username);

        if (!correspondingUser) throw new Error('not_found: username = ' + username);

        return await this.createSession(null, correspondingUser);
    }

    /**
     * @public
     *
     * @description Signs up a new user.
     *
     * @param {string} accessToken - The OAuth2 access token with the 'session:create' permission that identifies the
     * user to sign up. Must be encoded.
     *
     * @param {boolean} shouldLogin - Whether to login the user after signup.
     *
     * @throws {Error} If the manager is not initialized.
     *
     * @return {Promise<Session|void>} A promise that resolves when the operation finishes and returns the session
     * or `void` if `shouldLogin` was `false`.
     */
    async signup(accessToken, shouldLogin) {
        this.throwIfNotInitialized();

        const decodedToken = await this.verifyAccessToken(accessToken);

        let userInfo = await this.fetchUserInfo(accessToken);

        let newUser = await userDao.commit(new User({username: userInfo.email, role: 'member'}));

        if (shouldLogin) {
            return await this.createSession(accessToken, newUser);
        }
    }

    /**
     * @public
     *
     * @description Signs up a new user without a password.
     *
     * @param {string} username - The new user's username.
     *
     * @throws {Error} If the manager is not initialized.
     *
     * @return {Promise<Session|void>} A promise that resolves when the operation finishes and returns a session for
     * this user.
     */
    async signupUnsafe(username) {
        this.throwIfNotInitialized();

        let existingUser = userDao.get(username);

        if (existingUser) throw new Error('user_exists');

        let newUser = await userDao.commit(new User({username: userInfo.email, role: 'member'}));

        return await this.createSession(accessToken, newUser);
    }

    /**
     * @public
     *
     * @description Returns the session that corresponds to the provided uuid.
     *
     * @param {string} uuid - The session's UUID.
     * @param {boolean} [renew = true] - Whether to renew the session (i.e. push back expiration) before returning it.
     *
     * @returns {Promise<SessionInfo|undefined>} The session info if it's found. `undefined` otherwise.
     *
     * @throws {Error} If the manager is not initialized.
     */
    async getSession(uuid, renew = true) {
        this.throwIfNotInitialized();

        let session = await sessionDao.get({uuid});

        if (!session) return undefined;

        if (renew) this.renewSession(session);

        return session;
    }

    createMiddleware() {
        return async (req, res, next) => {

            if (this.testUser) {
                req.user = this.testUser;
                return next();
            }

            const authToken = req.headers[AuthTokenHeader];

            if (!authToken) return next();

            let session = await this.getSession(authToken);

            if (session) req.user = session.user;

            next();
        }
    }

    /**
     * @private
     *
     * @description Check if the accessToken is valid and well formed.
     *
     * @param {string} accessToken - The accessToken (should be a JWT).
     *
     * @see https://github.com/auth0/node-jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback
     *
     * @returns {Promise<object>} A promise that resolves when the process is finished and returns the
     * decoded access token.
     */
    verifyAccessToken(accessToken) {

        return new Promise((resolveFn, rejectFn) => {

            jwt.verify(
                accessToken,
                this.oAuthAppPublicKey,
                {
                    issuer: this.oauthAppConfigs.issuer,
                    audience: this.oauthAppConfigs.audience,
                    algorithms: this.oauthAppConfigs.signingAlgorithms,
                },
                (error, decodedAccessToken) => {

                    if (error) return rejectFn(error);
                    resolveFn(decodedAccessToken);
                }
            );
        });
    }

    /**
     * @private
     *
     * @description Gets the public key from OAuth trusted server to verify access tokens.
     *
     * @returns {Promise<string>} A promise that resolves when the key is retrieved from the OAuth server or from
     * the local cache.
     *
     * @see https://github.com/auth0/node-jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback
     */
    getSigningKeyFromOAuthServer() {

        return new Promise((resolveFn, rejectFn) => {

            const client = jwksClient({ jwksUri: this.oauthAppConfigs.publicKeyUrl });

            // TODO: the keyId can be retrieved using jwksClient.getKeys().
            // Maybe we can just use the first key retrieved using that (log warning if more than one key)
            // But it's good to let the developer overwrite the keyId, so let him.

            client.getSigningKey(this.oauthAppConfigs.keyId, (error, key) => {

                if (error) return rejectFn(error);
                if (!key) return rejectFn(new Error('Singing key returned by OAuth2 server is undefined!'));

                resolveFn(key.publicKey || key.rsaPublicKey);
            });
        });
    }


    /**
     * @private
     *
     * @description Creates and starts a new session for the provided user.
     *
     * @param {string} accessToken - The access token (still encoded).
     *
     * @param {User} user - The user for whom this session is for.
     *
     * @returns {Promise<SessionInfo>} A new activated session for this user.
     */
    async createSession(accessToken, user) {

        let retryAttempts = 0;
        let sessionUuid = uuid();

        while (await sessionDao.get({uuid: sessionUuid})) {

            retryAttempts += 1;

            if (retryAttempts >= MaxUuidRetries)
                throw new Error(`Could not generate session UUID! Max retries (${MaxUuidRetries}) reached.`);

            sessionUuid = uuid();
        }

        return await sessionDao.commit(new SessionInfo({
            user: user,
            uuid: sessionUuid,
            rawSessionAccessToken: accessToken,
            expirationTime: new Date(Date.now() + SessionTimeToLive),
        }));
    }

    /**
     * @private
     *
     * @description Renews the provided session (push back the session's expiration time).
     *
     * @param {SessionInfo} session - The session to renew.
     */
    renewSession(session) {

        session.expirationTime = new Date(Date.now() + SessionTimeToLive);

        sessionDao.commit(session); // do it async, but don't await

        return session;
    }


    /**
     * @private
     * @description Fetches from OAuth2 server the user info associated to the user that generated the access token.
     *
     * @param {string} accessToken - The encoded access token that identifies the user.
     *
     * @returns {Promise<{email:string}>} A promise that resolves when the user info is returned from the server and
     * returns that user info. Currently, the user info only contains the user's email.
     *
     * @throws {Error} If the response code returned by the OAuth2 server is not 200 -OR-
     * If anything else doesn't go to smoothly. No way of knowing what error will be thrown.
     */
    fetchUserInfo(accessToken) {

        return new Promise((resolveFn, rejectFn) => {

            const options = {
                method: 'GET',
                url: this.oauthAppConfigs.userInfoUrl,
                headers: {'Authorization': `Bearer ${accessToken}`}
            };

            request(options, (error, response, body) => {

                if (error) return rejectFn(error);
                if (!response) return rejectFn(`Response is undefined while fetching user info for access token '${accessToken}'.`);

                if (response.statusCode === 200) {

                    let userInfo = JSON.parse(body);

                    if (typeof(userInfo.email) !== 'string')
                        return rejectFn(`User info does not contain email for access token '${accessToken}'.`);

                    return resolveFn(userInfo);
                }

                rejectFn(new Error(
                    `Unknown response code (${response.statusCode}) while fetching user info for access token '${accessToken}'.`
                ));
            });
        });
    }

    /**
     * @private
     *
     * @description Deletes the session associated to the provided uuid.
     *
     * @param {string} uuid - The uuid of the session to delete.
     */
    async deleteSession(uuid) {
        await sessionDao.delete(uuid);
    }

    /**
     * @private
     * @description Throws an error if this manager is not initialized.
     */
    throwIfNotInitialized() {
        if (!this.isInitialized) throw new Error(
            `${AuthenticationManager.name} is not initialized! Call ${AuthenticationManager.name}.initialize() first.`
        );
    }
}

module.exports = new AuthenticationManager(CurrentOAuthConfigs);


/**
 * @typedef {Object} OAuthAppConfigs
 *
 * @property {string} issuer - URL/id of the issuer of the JWTs.
 *
 * @property {string[]} signingAlgorithms - The algorithms used to encode the JWT.
 *
 * @property {string} audience - Identifies the recipients that the JWT is intended for.
 *
 * @property {string} keyId - Id of the public key used to validate OAuth2 messages (aka Kid). Can be obtained by
 * visiting the {@link publicKeyUrl} in your browser or using {@link JwksClient.getKeys}.
 *
 * @property {string} publicKeyUrl - Url to use to get the message validation public key on OAuth2 server.
 *
 * @property {string} userInfoUrl - URL used to retrieve user info (i.e. email) of a user given an access token.
 *
 * @property {string} userSettingsUrl - Address at which we can update a user's profile (e.g. block his account).
 *
 *
 * @see https://auth0.com/docs/api-auth/tutorials/verify-access-token#validate-the-claims
 */