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

const DeleteMealEndpoint = "/meals";
const TestDatabaseConfigs = {host: 'localhost', database: 'calories-test'};

const mealDao = new MealDao();

describe('calories/meals/rest-api/meal-endpoints DELETE', () => {

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

    context('DELETE /meals', () => {

        // After tests of "DELETE /meals", remove all meals from test database.
        after(() => MealModel.deleteMany());

        it('should delete a meal that is in the database', async () => {

            // Clear database
            await MealModel.deleteMany();

            let mealToDelete = await mealDao.commit(new Meal({
                calories: 100,
                time: new Date(),
                name: 'Some meal',
                userId: AuthManager.testUser.id,
            }));

            await new Promise((resolve, reject) => {

                httpClient.delete(url(`${DeleteMealEndpoint}/${mealToDelete.id}`), (error, response, body) => {

                    if (error) return reject(error);

                    if (response.statusCode !== 200)
                        return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                    resolve();
                });
            });

            let allMeals = await mealDao.search();

            expect(allMeals.length).to.equal(0, 'Meal was not deleted!');
        });
    });
});