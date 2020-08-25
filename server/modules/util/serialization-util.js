/**
 * @description Util namespace for serialization and deserialization.
 */
class SerializationUtil {

    /**
     * @description Converts a string to a javascript floating point number. Unlike parseFloat(), this function will
     * return a number or undefined. If the given string is undefined or an invalid number string, undefined is returned.
     *
     * @param {string|undefined} numberString - The string to convert.
     *
     * @returns {number|undefined} The provided string as a number or undefined if the argument was undefined or an
     * invalid number string.
     */
    static stringToNumber(numberString) {

        const number = parseFloat(numberString);

        return Number.isNaN(number)? undefined : number;
    }

    /**
     * @description Converts a string to a Date object instance. Unlike Date.parse(), this function will return a Date object, not a number of
     * milliseconds. If the given string is undefined or an invalid date string, undefined is returned. If the argument
     * is already a Date, the same date will be returned.
     *
     * @param {string|Date} [dateString] - The string to convert. If already a Date, no problem.
     *
     * @returns {Date|undefined} The provided string as a Date object or undefined if the argument was undefined or an invalid date.
     */
    static stringToDate(dateString) {

        // Although we don't HAVE to do the following check, it makes the code clearer and avoids doing
        // serialization and deserialization for no reason
        if (dateString instanceof Date) return dateString; // Already a date? Return it.

        const date = Date.parse(dateString);

        return Number.isNaN(date)? undefined : new Date(date);
    }

    /**
     * @description Converts a string to an array object. If the given string is undefined, undefined is returned.
     *
     * @param {string} [arrayString] - The string to convert.
     *
     * @returns {Array|undefined} The provided string as an array or undefined if the provided string is undefined.
     *
     * TODO BUG : Doesn't escape commas! (basically, only works for values that don't contain commas.)
     */
    static stringToArray(arrayString) {

        if (!arrayString || arrayString === "") return [];

        return arrayString? arrayString.split(',') : undefined;
    }
}

module.exports = SerializationUtil;