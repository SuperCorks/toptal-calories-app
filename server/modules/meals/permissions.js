const {AccessControl} = require('role-acl');
const ac = new AccessControl();

// ADMIN PERMISSIONS
// Admins can CRUD all records
ac.grant('admin').execute('*').on('meals');
ac.grant('admin').execute('*').on('meal');

// MANAGER PERMISSIONS
// Managers can't do anything on meals

// MEMBER PERMISSIONS
// Members can see their own meals
ac.grant('member').condition({Fn: 'EQUALS', args: {reqUserId: '$.mealUserIdFilter'}}).execute('read').on('meals');

// Members can commit/delete their own meal
for (let action of ['commit','delete'])
    ac.grant('member').condition({Fn: 'EQUALS', args: {reqUserId: '$.mealUserId'}}).execute(action).on('meal');

module.exports = ac;