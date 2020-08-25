const httpClient = require('request');
const {expect, fail, assert} = require('test-utils/functions');
const {url} = require('test-utils/request-util');

const AuthManager = require('calories/authentication/manager/authentication-manager');

const server = require('../../bin/server');

const MongoConfigs = require('../../mongo-configs');
const Meal = require('calories/meals/entities/meal');
const User = require('calories/users/entities/user');
const MealDao = require('calories/meals/daos/meal-dao');
const {MealModel} = require('calories/meals/storage/mongoose-meal');

const CreateMealEndpoint = "/meals";
const TestDatabaseConfigs = {host: 'localhost', database: 'calories-test'};

const mealDao = new MealDao();

describe('calories/meals/rest-api/meal-endpoints POST', () => {

    let serverInstance;

    before(async () => {

        AuthManager.testUser = new User({id: 'a1', role: 'admin'});

        MongoConfigs.current = TestDatabaseConfigs;

        serverInstance = await server.start();
    });

    after(() => {

        AuthManager.testUser = undefined;

        MongoConfigs.reset();
        return serverInstance.close();
    });

    context('POST /meals', () => {

        // After tests of "POST /meals", remove all meals from test database.
        after(() => MealModel.deleteMany());

        it('should create a new meal when meal has no id', async () => {

            await new Promise((resolve, reject) => {

                let meal = new Meal({
                    time: new Date(),
                    calories: 213423,
                    name: 'some meal Z',
                    userId: AuthManager.testUser.id,
                });

                httpClient.post(url(CreateMealEndpoint, {body: meal}), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        let mealId = body.id;
                        expect(mealId).to.be.a('string', `Returned meal.id isn't a string.`);

                        let returnedMeal = new Meal(body);
                        returnedMeal.id = undefined; // clear id for comparison

                        expect(returnedMeal).to.eql(meal, `Returned meal isn't the same as sent meal.`);

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });
        });

        it('should update the meal when meal has an id', async () => {

            // Clear database
            await MealModel.deleteMany();

            let originalMeal = await mealDao.commit(new Meal({
                calories: 132,
                time: new Date(),
                name: "Some meal ABC",
                userId: "jifgw320329jd",
            }));

            await new Promise((resolve, reject) => {

                // Copy meal
                let updatedMeal = Object.assign({}, originalMeal);

                // Make a few modifications
                updatedMeal.calories += 500;
                updatedMeal.name += '(+ potatoes)';

                httpClient.post(url(CreateMealEndpoint, {body: updatedMeal}), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        let returnedMeal = new Meal(body);

                        expect(returnedMeal.id).to.equal(updatedMeal.id,
                            `Returned meal id isn't the same as sent meal id!`
                        );

                        expect(returnedMeal).to.eql(updatedMeal, `Returned meal isn't the same as sent meal.`);
                        expect(returnedMeal).to.not.eql(originalMeal,
                            `Returned meal isn't different than the original meal even after being modified.`);

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });
        });
    });
});