const disc = require("discord.js");
const hb = require("hastebin.js");
const bin = new hb();

module.exports.send = (client, error) => {
    client.channels.find("id", "552052555874828288").startTyping();
    var e = new disc.RichEmbed();
    e.setTitle(":x: Error occured!");
    e.setDescription(e);
    e.setColor("RED");

    if (e.stack) {
        bin.post(e.stack).then(link => {
            e.setFooter("Full error stack can be viewed (here)[" + link + "].");
            client.channels.find("id", "552052555874828288").stopTyping();
            client.channels.find("id", "552052555874828288").send({ embed: e });
        })
    } else {
        bin.post(e+"\nThat's all there is.").then(link => {
            e.setFooter("Full error stack can be viewed (here)[" + link + "].");
            client.channels.find("id", "552052555874828288").stopTyping();
            client.channels.find("id", "552052555874828288").send({ embed: e });
        });
    }
}