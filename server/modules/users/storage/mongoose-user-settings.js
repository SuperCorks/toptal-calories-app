const mongoose = require('mongoose');

/** @see {UserSettings} */
const UserSettingsSchema = new mongoose.Schema({

    userId: Number,
    dailyCalories: Number,

}, {_id: false});

module.exports = {
    UserSettingsSchema,
};