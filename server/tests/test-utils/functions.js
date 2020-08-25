const sinon = require('sinon');
const chai = require('chai');
const expect = chai.expect;
const should = chai.should();
const chaiFail = chai.assert.fail;

/**
 * @description Fails a test if the provided reason is not undefined. You could also use mocha's DoneFn but then
 * you'd have to call it at the end of the test if it's not an async test.
 *
 * @param {string|Error|undefined} reason - The reason why the test failed. If undefined, this won't make the test
 * fail.
 */
function fail(reason) {
    if (reason) chaiFail(null, null, reason.toString());
    else chaiFail();
}

module.exports = {
    expect,
    spy: sinon.spy,
    should,
    fail,
    assert: chai.assert
};