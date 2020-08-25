const mongoose = require('mongoose');
const MongoConfigs = require('./mongo-configs');

const User = require('calories/users/entities/user');
const Meal = require('calories/meals/entities/meal');

async function seed() {

    console.log(`\n___ SEEDING DATA ___\n`);

    console.log(`Connecting to mongodb...`);
    // init mongoose
    await mongoose.connect(
        `mongodb://${MongoConfigs.current.host}/${MongoConfigs.current.database}`,
        { useNewUrlParser: true }
    );
    console.log(`OK!\n`);

    const UserDao = require('calories/users/daos/user-dao');
    const userDao = new UserDao();

    const MealDao = require('calories/meals/daos/meal-dao');
    const mealDao = new MealDao();

    console.log(`Creating fake accounts...`);
    await userDao.commit(new User({username: 'fake-admin@example.com', role: 'admin'}));
    await userDao.commit(new User({username: 'fake-manager@example.com', role: 'manager'}));
    let fakeMember = await userDao.commit(new User({username: 'fake-member@example.com', role: 'member', settings: {dailyCalories: 800}}));
    console.log('OK!\n');

    console.log(`Creating real accounts...`);
    await userDao.commit(new User({username: 'simoncorcos.ing@gmail.com', role: 'admin'}));
    console.log('OK!\n');

    console.log(`Creating a few meals...`);
    await mealDao.commit(new Meal({userId: fakeMember.id, name: "Breakfast (bacon + eggs)", calories: 600, time: new Date(2020, 1, 1, 8, 0, 0)}));
    await mealDao.commit(new Meal({userId: fakeMember.id, name: "Lunch (burger)", calories: 450, time: new Date(2020, 1, 1, 13, 0, 0)}));
    await mealDao.commit(new Meal({userId: fakeMember.id, name: "Diner (steak + potato)", calories: 900, time: new Date(2020, 1, 1, 20, 0, 0)}));
    console.log('OK!\n');

    console.log(`Disconnecting mongodb...`);
    await mongoose.disconnect();
    console.log(`\nDONE!`);
}

seed().catch(console.error);