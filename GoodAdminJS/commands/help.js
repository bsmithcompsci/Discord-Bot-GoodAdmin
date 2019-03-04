const disc = require("discord.js");
const fs = require("fs");
const api = require("../data/core/api/embeds.js");

module.exports.details = { name: "Help", usage: "!help", catagory: 1, sdesc: "Shows all commands currently registered." };
module.exports.execute = (client, msg, args, odata) => {
    var e = new disc.RichEmbed()
    e.setTitle(":pencil: Commands");
    var general = "";
    var moderation = "";
    var misc = "";
    //Log for debug :(odata)
    for (i in odata.cmds) {
        //Log for debug :(odata.cmds[i]);
        var command = odata.cmds[i].data.details;

        if (command.catagory == 1) {
            general += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        } else if (command.catagory == 2) {
            moderation += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        } else if (command.catagory == 3) {
            misc += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        }

        if (i > odata.cmds.length || i == odata.cmds.length || i == odata.cmds.length - 1) {
            //Log for debug :("Finished.")
            if (general == "") {
                general = "None registered.";
            }
            if (moderation == "") {
                moderation = "None registered.";
            }
            if (misc == "") {
                misc = "None registered.";
            }

            e.addField(":bookmark: General Commands:", general);
            e.addField(":shield: Moderation Commands:", moderation);
            e.addField(":stuck_out_tongue_winking_eye: Misc Commands:", misc);
            e.setColor("BLUE");
            msg.author.send({ embed: e }).then(mess => { msg.channel.send({ embed: api.senttodms("help") }); if (msg.deleteable) { msg.delete() }; setTimeout(function () { mess.delete() }, 120000); }).catch(er => {
                e.setFooter("Your DM settings prevented me from sending it to your dm's, so the help was sent in the channel.");
                msg.channel.send({ embed: e }).then(mess => {
                    setTimeout(function () {
                        mess.delete()
                    }, 60000)
                });
            });
        }
    }
}