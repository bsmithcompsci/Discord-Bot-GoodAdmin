module.exports.execute = (client) => {
    //Log for debug :("Javascript version of GoodAdmin loaded and ready!")
    client.user.setActivity("!help | " + client.guilds.array().length + " guild(s)", { type: "WATCHING" });
}