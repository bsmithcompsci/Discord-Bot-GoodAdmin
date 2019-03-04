const disc = require("discord.js");
const error = require("../data/core/api/errors.js");

module.exports.send = (client, guild, actions) => {
    if (!client || !guild || !actions)
        return error.send(client, "No actions, guild or client provided. Something went wrong. LOGGING.JS | MISSING PARAMS");


}