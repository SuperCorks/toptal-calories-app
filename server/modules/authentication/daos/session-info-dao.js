/** @description Data access object for {@link SessionInfo} entities. */
class SessionInfoDao {

    constructor() {
        /**
         * @private
         * @readonly
         */
        this.storedSessions = {};
    }

    /**
     * @description Retrieves a single session record from storage.
     *
     * @param {object} options - Params object.
     * @param {string} options.uuid - The session's uuid.
     *
     * @returns {Promise<SessionInfo|undefined>}
     */
    async get(options) {

        if (options.uuid) return Promise.resolve(this.storedSessions[options.uuid]);

        return Promise.resolve(undefined);
    }

    /**
     * @description Creates or updates a session record in storage.
     *
     * @param {SessionInfo} session - The session to commit to storage.
     *
     * @return {Promise<SessionInfo>} The committed session.
     */
    async commit(session) {

        if (!session) throw new Error('null_param: session');
        if (!session.uuid) throw new Error('invalid_param: session.uuid');

        this.storedSessions[session.uuid] = session;

        return Promise.resolve(session);
    }

    /**
     * @description Deletes the record associated to the session's uuid.
     *
     * @param {string} uuid - The id of the session to delete.
     *
     * @return {Promise<void>} A promise that resolves when the operation finishes.
     */
    async delete(uuid) {
        delete this.storedSessions[uuid];
        return Promise.resolve();
    }
}

module.exports = SessionInfoDao;