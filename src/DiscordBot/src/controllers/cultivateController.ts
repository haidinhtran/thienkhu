import { ChatInputCommandInteraction, ButtonInteraction, ActionRowBuilder, ButtonBuilder, ButtonStyle } from 'discord.js';
import { embedBuilder } from '../utils/embedBuilder.js';
import { logger } from '../utils/logger.js';
import { apiClient } from '../api/CultivationApiClient.js';

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

    if (action === 'profile') {
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';
        const username = interaction.user.username;

        const profile = await apiClient.getCharacterProfile(discordId, serverId, username);
        const profileEmbed = embedBuilder.buildProfileEmbed(profile);

        await interaction.editReply({ embeds: [profileEmbed], components: [] });
      } catch (error) {
        logger.error(`Error fetching profile: ${error}`, { traceId });
        await interaction.editReply({ content: 'An error occurred while fetching your profile. Please try again later.' });
      }
    } else {
      await interaction.editReply({ content: `You clicked ${action}. This feature is coming soon!` });
    }
  }
};
