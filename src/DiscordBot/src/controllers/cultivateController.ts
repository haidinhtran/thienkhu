import { ChatInputCommandInteraction, ButtonInteraction, ActionRowBuilder, ButtonBuilder, ButtonStyle } from 'discord.js';
import { embedBuilder } from '../utils/embedBuilder.js';
import { logger } from '../utils/logger.js';

export const handleCultivateInteraction = async (interaction: ChatInputCommandInteraction | ButtonInteraction, traceId: string) => {
  await interaction.deferReply({ ephemeral: false });
  logger.info(`Handling cultivate interaction for ${interaction.user.tag}`, { traceId });

  if (interaction.isChatInputCommand()) {
    // Main menu
    const embed = embedBuilder.buildMainMenuEmbed(interaction.user.username);
    
    const row = new ActionRowBuilder<ButtonBuilder>()
      .addComponents(
        new ButtonBuilder()
          .setCustomId('cultivate_profile')
          .setLabel('Profile')
          .setStyle(ButtonStyle.Primary),
        new ButtonBuilder()
          .setCustomId('cultivate_explore')
          .setLabel('Exploration')
          .setStyle(ButtonStyle.Success),
        new ButtonBuilder()
          .setCustomId('cultivate_domain')
          .setLabel('Secret Domain')
          .setStyle(ButtonStyle.Danger)
      );

    await interaction.editReply({ embeds: [embed], components: [row] });
  } else if (interaction.isButton()) {
    const action = interaction.customId.replace('cultivate_', '');
    logger.info(`Cultivate button clicked: ${action}`, { traceId });
    
    // Stub implementation
    await interaction.editReply({ content: `You clicked ${action}. This feature is coming soon!` });
  }
};
