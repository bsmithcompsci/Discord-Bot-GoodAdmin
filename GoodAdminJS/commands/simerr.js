module.exports.details = { hidden: true }

const error = require("../data/core/api/errors.js");
module.exports.execute = (client, msg) => {
    msg.channel.send("").then(mess => { console.log(mess.content) }).catch(e => {
        error.send(client, e);
    });
}