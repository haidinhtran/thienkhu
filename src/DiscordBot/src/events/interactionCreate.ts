import { Events, Interaction } from 'discord.js';
import { logger } from '../utils/logger.js';
import { embedBuilder } from '../utils/embedBuilder.js';
import { handleCultivateInteraction } from '../controllers/cultivateController.js';

const generateTraceId = () => Math.random().toString(36).substring(2, 15);

export const interactionCreateEvent = {
  name: Events.InteractionCreate,
  async execute(interaction: Interaction) {
    const traceId = generateTraceId();

    try {
      if (interaction.isChatInputCommand()) {
        if (interaction.commandName === 'cultivate') {
          await handleCultivateInteraction(interaction, traceId);
        }
      } else if (interaction.isButton()) {
        if (interaction.customId.startsWith('cultivate_')) {
          await handleCultivateInteraction(interaction, traceId);
        }
      }
    } catch (error) {
      logger.error('Error handling interaction', { traceId, error });
      
      const errorEmbed = embedBuilder.buildErrorEmbed('An unexpected error occurred.', traceId);
      
      if (interaction.isRepliable()) {
        if (interaction.deferred || interaction.replied) {
          await interaction.followUp({ embeds: [errorEmbed], ephemeral: true });
        } else {
          await interaction.reply({ embeds: [errorEmbed], ephemeral: true });
        }
      }
    }
  }
};
