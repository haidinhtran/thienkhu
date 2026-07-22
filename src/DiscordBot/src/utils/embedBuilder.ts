import { EmbedBuilder } from 'discord.js';

export const embedBuilder = {
  buildErrorEmbed: (message: string, traceId: string) => {
    return new EmbedBuilder()
      .setColor('Red')
      .setTitle('Error Occurred')
      .setDescription(message)
      .setFooter({ text: `Trace ID: ${traceId}` });
  },

  buildMainMenuEmbed: (username: string) => {
    return new EmbedBuilder()
      .setColor('Blue')
      .setTitle('Cultivation Main Menu')
      .setDescription(`Welcome back, ${username}. What would you like to do?`);
  },

  buildProfileEmbed: (profile: any) => {
    return new EmbedBuilder()
      .setColor('Gold')
      .setTitle(`${profile.username}'s Profile`)
      .addFields(
        { name: 'Realm', value: `${profile.realmName} (Lv. ${profile.level})`, inline: true },
        { name: 'Qi', value: `${profile.currentQi} / ${profile.dailyQiLimit}`, inline: true },
        { name: 'Spirit Stones', value: `${profile.spiritStones}`, inline: true },
        { name: 'Status', value: `${profile.currentState}`, inline: false }
      );
  }
};
