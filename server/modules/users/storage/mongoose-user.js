

const mongoose = require('mongoose');
const {UserSettingsSchema} = require('./mongoose-user-settings');

/** @see {User} */
const UserSchema = new mongoose.Schema({
    // id is created automatically by mongoose and mapped to mongodb _id.

    username: {type: String, lowercase: true, unique: true},
    role: {type: String, lowercase: true, default: 'member'},
    settings: {type: UserSettingsSchema, required: false}
});

module.exports = {
    UserSchema,
    UserModel: mongoose.model('users', UserSchema),
};