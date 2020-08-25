const mongoose = require('mongoose');

/** @see {Meal} */
const MealSchema = new mongoose.Schema({
    // id is created automatically by mongoose and mapped to mongodb _id.

    userId: {type: String, required: true},
    name: {type: String},
    calories: {type: Number, default: 0},
    time: {type: Date, default: Date.now()}
});

module.exports = {
    MealSchema,
    MealModel: mongoose.model('meals', MealSchema),
};