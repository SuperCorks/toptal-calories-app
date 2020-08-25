const {expect, fail, assert} = require('test-utils/functions');

const mealPermissions = require('calories/meals/permissions');

describe('calories/meals/permissions', () => {

    context('read:meals', () => {

        it('admins should be able to CRUD all meals', async () => {

            assert((await mealPermissions.can('admin').execute('read').on('meals')).granted, `Admin can't read meals!`);
        });

        it(`members should be able to read their own meals`, async () => {

            for (let invalidContext of [
                undefined,
                {mealUserIdFilter: 'a1', reqUserId: 'a2'},
                {mealUserIdFilter: 'a1'}
            ]) {

                assert(
                    (await mealPermissions.can('member').context(invalidContext).execute('read').on('meals')).granted === false,
                    `Members shouldn't be able to read meals when context is: ${JSON.stringify(invalidContext)} !`
                );
            }

            let validContext = {mealUserIdFilter: 'a1', reqUserId: 'a1'};
            assert(
                (await mealPermissions.can('member').context(validContext).execute('read').on('meals')).granted,
                `Members should be able to read meals when context is: ${JSON.stringify(validContext)} !`
            );
        });
    });

    context('commit:meal', () => {

        it('admins should be able to CRUD all meals', async () => {

            assert((await mealPermissions.can('admin').execute('commit').on('meal')).granted, `Admin can't commit:meal !`);
        });

        it(`members should be able to commit their own meals`, async () => {

            for (let invalidContext of [
                undefined,
                {mealUserId: 'a1', reqUserId: 'a2'},
                {mealUserId: 'a1'}
            ]) {

                assert(
                    (await mealPermissions.can('member').context(invalidContext).execute('commit').on('meal')).granted === false,
                    `Members shouldn't be able to commit meals when context is: ${JSON.stringify(invalidContext)} !`
                );
            }

            let validContext = {mealUserId: 'a1', reqUserId: 'a1'};
            assert(
                (await mealPermissions.can('member').context(validContext).execute('commit').on('meal')).granted,
                `Members should be able to commit a meal when context is: ${JSON.stringify(validContext)} !`
            );
        });
    });

    context('delete:meal', () => {

        it('admins should be able to CRUD all meals', async () => {

            assert((await mealPermissions.can('admin').execute('delete').on('meal')).granted, `Admin can't delete:meal!`);
        });

        it(`members should be able to delete their own meal`, async () => {

            // Invalid contexts
            const unauthorizedContexts = [
                undefined,
                {userRole: 'member'},
                {mealUserId: 'a1', reqUserId: 'b2'},
                {mealUserId: 'a1'}
            ];

            for (let context of unauthorizedContexts){

                assert(
                    (await mealPermissions.can('member').context(context).execute('delete').on('meal')).granted === false,
                    `Members shouldn't be able to delete a meal when context is: ${JSON.stringify(context)} !`
                );
            }

            // Valid contexts
            let validContext = {mealUserId: 'a1', reqUserId: 'a1'};
            assert(
                (await mealPermissions.can('member').context(validContext).execute('delete').on('meal')).granted,
                `Members should be able to delete a meal when context is: ${JSON.stringify(validContext)} !`
            );
        });
    });
});