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
  }
};
