const Serializer = require('calories/util/serialization-util');

class Meal {

    /** @param {Meal} init - Initial values. */
    constructor(init) {

        /**
         * @description The unique server-side id of this record.
         * @type {number}
         */
        this.id = init.id;

        /**
         * @description The id of the user that owns this record.
         * @type {string}
         */
        this.userId = init.userId;

        /**
         * @description The name of the meal (e.g. 'lunch' or 'General Tao') .
         * @type {string}
         */
        this.name = init.name;

        /**
         * @description The meal's number of calories.
         * @type {number}
         */
        this.calories = init.calories;

        /**
         * @description The meal's date and time (i.e. when it was eaten).
         * @type {Date}
         */
        this.time = Serializer.stringToDate(init.time);
    }
}

module.exports = Meal;