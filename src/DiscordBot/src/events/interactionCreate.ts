import { Events, Interaction, MessageFlags } from 'discord.js';
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
      } else if (interaction.isStringSelectMenu()) {
        if (interaction.customId.startsWith('cultivate_')) {
          await handleCultivateInteraction(interaction, traceId);
        }
      }
    } catch (error: any) {
      logger.error('Error handling interaction', { traceId, error });
      
      // Error 10062: Unknown Interaction (token expired)
      if (error?.code === 10062) {
        logger.warn(`Interaction token expired for trace: ${traceId}. Silently logging.`);
        return;
      }
      
      let errorMessage = 'An unexpected error occurred. Your session may have expired. Please run `/cultivate` again.';
      if (error?.name === 'ApiBusinessError') {
        errorMessage = error.message;
      }
      
      const errorEmbed = embedBuilder.buildErrorEmbed(errorMessage, traceId);
      
      if (interaction.isRepliable()) {
        try {
          if (interaction.deferred || interaction.replied) {
            await interaction.followUp({ embeds: [errorEmbed], flags: MessageFlags.Ephemeral });
          } else {
            await interaction.reply({ embeds: [errorEmbed], flags: MessageFlags.Ephemeral });
          }
        } catch (followUpError) {
          logger.error('Failed to send fallback error message', { traceId, followUpError });
        }
      }
    }
  }
};
