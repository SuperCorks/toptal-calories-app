/** @description User settings for the calories app. */
class UserSettings {

    /** @param {UserSettings} init - Initial values. */
    constructor(init) {

        /**
         * @description The daily calorie threshold of the user.
         * @type {number}
         */
        this.dailyCalories = init.dailyCalories;
    }
}

module.exports = UserSettings;