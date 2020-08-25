const UserSettings = require('./user-settings');

class User {

    /** @param {User} init - Initial values. */
    constructor(init) {

        /**
         * @description The unique server-side id of this record.
         * @type {string}
         */
        this.id = init.id;

        /**
         * @description The user's username.
         * @type {string}
         */
        this.username = init.username;

        /**
         * @description The user's role.
         * @type {UserRoles}
         */
        this.role = init.role || 'member';

        /**
         * @description The user's settings for the calories app.
         * @type {UserSettings}
         */
        this.settings = init.settings && new UserSettings(init.settings);
    }
}

module.exports = User;