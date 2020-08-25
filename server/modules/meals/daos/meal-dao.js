const Meal = require('../entities/meal');
const {MealModel} = require('../storage/mongoose-meal');

/** @description Data access object for {@link Meal} entities storage. */
class MealDao {

    /**
     * @description Retrieves a single {@link Meal} record from storage.
     *
     * @param {string} mealId - The meal's id.
     *
     * @returns {Promise<Meal|null>} The meal that corresponds to the id or `null` if no meal matches.
     */
    async get(mealId) {

        let foundMeal = await MealModel.findOne({_id: mealId});

        if (!foundMeal) return null;

        return new Meal(foundMeal);
    }

    /**
     * @description Retrieves {@link Meal} records from storage.
     *
     * @param {string|undefined} [userId] - The id of the user that owns the meals to return.
     *
     * @returns {Promise<Meal[]>}
     */
    async search(userId) {

        let filters = {};

        if (userId) filters = {userId};

        return (await MealModel.find(filters)).map(doc => new Meal(doc));
    }

    /**
     * @description Creates or updates a meal record in storage.
     *
     * @param {Meal} meal - The meal to commit to storage.
     *
     * @return {Promise<Meal>} A promise that resolves when the operation finishes and returns
     * the committed meal.
     */
    async commit(meal) {

        if (!meal) throw new Error('null_param: meal');

        if (meal.id) {

            let mealDoc = await MealModel.findOne({_id: meal.id});

            mealDoc = await mealDoc.overwrite(meal).save();

            return new Meal(mealDoc);

        } else {

            let mealDoc = await new MealModel(meal).save();

            return new Meal(mealDoc);
        }
    }

    /**
     * @description Deletes the record associated to the meal's id.
     *
     * @param {string} mealId - The id of meal to delete.
     *
     * @return {Promise} A promise that resolves when the operation completes.
     */
    delete(mealId) {

        if (!mealId) throw new Error('null_param: mealId');

        return MealModel.deleteOne({_id: mealId});
    }
}

module.exports = MealDao;