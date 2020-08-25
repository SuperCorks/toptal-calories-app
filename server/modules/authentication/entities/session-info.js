/** @description Stores info about an authentication session. */
class SessionInfo {

    /** @param {SessionInfo|{}} [init] - Initial values. */
    constructor(init = {}) {

        /**
         * @description The session's uuid. Acts as an opaque token.
         * @type {string}
         */
        this.uuid = init.uuid;

        /**
         * @description The session's authenticated user.
         * @type {User}
         */
        this.user = init.user;

        /**
         * @description The session's expiration date.
         * @type {Date}
         */
        this.expirationTime = init.expirationTime;

        /**
         * @description The encoded OAuth2 access token used to create the session this {@link SessionInfo}.
         * @type {string}
         */
        this.rawSessionAccessToken = init.rawSessionAccessToken;
    }

    /**
     * @description Whether this session is expired.
     * @return {boolean}
     */
    get isExpired() {
        return this.expirationTime.getTime() > Date.now();
    }
}

module.exports = SessionInfo;
