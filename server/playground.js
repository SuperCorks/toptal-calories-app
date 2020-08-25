const {AccessControl} = require('role-acl');
const ac = new AccessControl();

async function play() {

    // Admins can CRUD all records
    ac.grant('admin').execute('*').on('meals');
    ac.grant('admin').execute('*').on('meal');
    ac.grant('admin').execute('*').on('users');
    ac.grant('admin').execute('*').on('user');

    // Managers can CRUD members (only members).
    ac.grant('manager').execute('read').condition(context => {

        if (!Array.isArray(context.rolesFilter))
            throw new Error('invalid_argument: context.rolesFilter is not an array!');

        return !(context.rolesFilter.length !== 1 || context.rolesFilter[0] !== 'members');

    }).on('users');

    for (let action of ['commit', 'delete']) {

        ac.grant('manager').execute(action).condition(context => {

            if (!context.userRole)
                throw new Error('invalid_argument: context.userRole is missing!');

            return context.userRole === 'member';

        }).on('user');
    }

    // Members can CRUD their meals
    ac.grant('member').execute('read')
        .condition({Fn: 'EQUALS', args: {reqUserId: '$.mealsUserId'}})
        .on('meals');

    for (let action of ['commit','delete']) {
        ac.grant('member').execute(action)
            .condition({Fn: 'EQUALS', args: {reqUserId: '$.mealsUserId'}})
            .on('meals');
    }

    console.log((await ac.can('admin').execute('read').on('meals')).granted);   // true
    console.log((await ac.can('admin').execute('commit').on('meal')).granted);  // true
    console.log((await ac.can('admin').execute('delete').on('meal')).granted);  // true


    console.log((await ac.can('manager').execute('read').on('meals')).granted);  // false
    console.log((await ac.can('manager').execute('commit').on('meal')).granted); // false
    console.log((await ac.can('manager').execute('delete').on('meal')).granted); // false

    console.log((await ac.can('manager').execute('read').with({rolesFilter: ['admin']}).on('meals')).granted);  // true
    console.log((await ac.can('manager').execute('commit').on('meal')).granted); // false
    console.log((await ac.can('manager').execute('delete').on('meal')).granted); // false
}

play().catch(console.error);