module.exports.execute = (client) => {
    console.log("Javascript version of GoodAdmin loaded and ready!")
    client.user.setActivity("!help | " + client.guilds.array().length + " guild(s)", { type: "WATCHING" });
}