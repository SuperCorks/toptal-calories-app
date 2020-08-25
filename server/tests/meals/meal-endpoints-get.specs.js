const httpClient = require('request');
const {url} = require('test-utils/request-util');
const {expect, fail, assert} = require('test-utils/functions');

const AuthManager = require('calories/authentication/manager/authentication-manager');

const server = require('../../bin/server');

const MongoConfigs = require('../../mongo-configs');
const User = require('calories/users/entities/user');
const Meal = require('calories/meals/entities/meal');
const MealDao = require('calories/meals/daos/meal-dao');
const {MealModel} = require('calories/meals/storage/mongoose-meal');

const SearchMealsEndpoint = "/meals";
const TestDatabaseConfigs = {host: 'localhost', database: 'calories-test'};

const mealDao = new MealDao();

describe('calories/meals/rest-api/meal-endpoints-get GET', () => {

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

    context('GET /meals', () => {

        let allMeals = undefined;

        // Seed meals for tests of "GET /meals".
        before(async () => {

            let userId = AuthManager.testUser.id;

            allMeals = (await Promise.all([
                mealDao.commit(new Meal({userId: userId, calories: 100, name: 'Some meal', time: new Date()})),
                mealDao.commit(new Meal({userId: userId, calories: 500, name: 'Some other meal', time: new Date()})),
                mealDao.commit(new Meal({userId: userId, calories: 1430, name: 'Meal X', time: new Date()})),
                mealDao.commit(new Meal({userId: "fhui43h9hfnh3d", calories: 123540, name: 'Meal Y', time: new Date()})),
            ]));
        });

        // After tests of "GET /meals", remove all meals from test database.
        after(() => MealModel.deleteMany());

        it('should return all meals when no params', async () => {

            await new Promise((resolve, reject) => {

                httpClient.get(url(SearchMealsEndpoint), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        function byId(a, b) { return a < b ? -1 : 1; }

                        expect(body.sort(byId).map(m => new Meal(m))).to.eql(allMeals.sort(byId));

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });
        });

        it('should filter meals based on the userId query param', async () => {

            function byId(a, b) { return a < b ? -1 : 1; }

            for (let mealUserId of [AuthManager.testUser.id, 'fhui43h9hfnh3d']) {

                await new Promise((resolve, reject) => {

                    httpClient.get(url(`${SearchMealsEndpoint}?userId=${mealUserId}`), (error, response, body) => {

                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code when ${JSON.stringify(body)} given as body!`));

                        try {

                            let expectedMeals = allMeals.filter(m => m.userId === mealUserId);

                            expect(
                                body.sort(byId).map(m => new Meal(m)),
                                `Didn't return the right meals when the userId was: [${mealUserId}]`
                            ).to.eql(expectedMeals.sort(byId));

                            resolve();
                        } catch (e) {
                            reject(e);
                        }
                    });
                });
            }
        });
    });
});