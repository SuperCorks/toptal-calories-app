const Meal = require('../entities/meal');
const router = require('express').Router();
const MealDao = require('../daos/meal-dao');
const mealPermissions = require('../permissions');

const mealDao = new MealDao();

// search meals
router.get('/', async (req, res, next) => {

    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }

        const permission = await mealPermissions.can(req.user.role)
            .context({reqUserId: req.user.id, mealUserIdFilter: req.query.userId})
            .execute('read').on('meals');

        if (permission.granted) {

            const meals = await mealDao.search(req.query.userId);

            res.status(200).send(meals);

        } else {
            res.status(403).end();
        }

    } catch (e) { next(e); }
});

// commit a meal
router.post('/', async (req, res, next) => {
    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }

        const permission = await mealPermissions.can(req.user.role)
            .context({reqUserId: req.user.id, mealUserId: req.body.userId})
            .execute('commit').on('meal');

        if (permission.granted) {

            if (!req.body) return next(new Error('no_body'));

            let committedMeal = await mealDao.commit(new Meal(req.body));

            res.status(200).send(committedMeal);

        } else {
            res.status(403).end();
        }

    } catch (e) {
        next(e);
    }
});

// delete a meal
router.delete('/:mealId', async (req, res, next) => {
    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }


        const meal = await mealDao.get(req.params.mealId);

        if (!meal) {

            res.status(200).end();

            return next();
        }

        const permission = await mealPermissions.can(req.user.role)
            .context({reqUserId: req.user.id, mealUserId: meal.userId})
            .execute('delete').on('meal');

        if (permission.granted) {

            if (req.params.mealId === undefined) return next(new Error('no_meal_id'));

            await mealDao.delete(req.params.mealId);

            res.status(200).end();

        } else {
            res.status(403).end();
        }

    } catch (e) {
        next(e);
    }
});

module.exports = router;