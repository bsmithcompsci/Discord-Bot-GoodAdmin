const disc = require("discord.js");
const db = require("quick.db");

module.exports.execute = (client, odata, g) => {
    var e1 = new disc.RichEmbed();
    var e2 = new disc.RichEmbed();

    e1.setTitle(":heavy_plus_sign: Joined server!");
    e1.addField("GuildID", g.id);
    e1.addField("GuildName", g.name);
    e1.addField("Member Count", g.members.size);
    e1.addField("Owner", g.owner.user.tag);
    e1.setColor("GREEN");

    client.channels.find("id", "551655965129179146").send({ embed: e1 });

    e2.setTitle("Hello! I'm GoodAdmin! Thanks for adding me.");
    e2.setDescription("How do I get started?: **Say !help for commands**\n\nThank you for adding GoodAdmin to your server.\n- GoodAdmin Development Team");
    e2.setColor("GREEN");

    var channels = g.channels.array();
    for (i in channels) {
        if (channels[i].type == "text" && channels[i].name.includes("general")) {
            channels[i].send({ embed: e2 });
        }
    }
}