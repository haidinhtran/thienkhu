import { ChatInputCommandInteraction, ButtonInteraction, StringSelectMenuInteraction, ActionRowBuilder, ButtonBuilder, ButtonStyle, StringSelectMenuBuilder, StringSelectMenuOptionBuilder, EmbedBuilder } from 'discord.js';
import { embedBuilder } from '../utils/embedBuilder.js';
import { logger } from '../utils/logger.js';
import { apiClient } from '../api/CultivationApiClient.js';

export const handleCultivateInteraction = async (interaction: ChatInputCommandInteraction | ButtonInteraction | StringSelectMenuInteraction, traceId: string) => {
  await interaction.deferReply({ ephemeral: false });
  logger.info(`Handling cultivate interaction for ${interaction.user.tag}`, { traceId });

  if (interaction.isChatInputCommand()) {
    try {
      const discordId = interaction.user.id;
      const serverId = interaction.guildId || 'DM';
      const username = interaction.user.username;

      const profile = await apiClient.getCharacterProfile(discordId, serverId, username);

      if (!profile) {
        const embed = embedBuilder.buildWelcomeEmbed(username);
        const row = new ActionRowBuilder<ButtonBuilder>()
          .addComponents(
            new ButtonBuilder()
              .setCustomId('cultivate_begin_journey')
              .setLabel('Begin Journey')
              .setStyle(ButtonStyle.Primary)
          );
        await interaction.editReply({ embeds: [embed], components: [row] });
        return;
      }

      // Main menu
      const embed = embedBuilder.buildMainMenuEmbed(username);

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
            .setStyle(ButtonStyle.Danger),
          new ButtonBuilder()
            .setCustomId('cultivate_inventory')
            .setLabel('Inventory')
            .setStyle(ButtonStyle.Secondary)
        );

      await interaction.editReply({ embeds: [embed], components: [row] });
    } catch (error) {
      logger.error(`Error checking profile: ${error}`, { traceId });
      await interaction.editReply({ content: 'An error occurred while loading the menu.', components: [] });
    }
  } else if (interaction.isButton()) {
    const action = interaction.customId.replace('cultivate_', '');
    logger.info(`Cultivate button clicked: ${action}`, { traceId });

    if (action === 'begin_journey') {
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';
        const username = interaction.user.username;

        await apiClient.createCharacter(discordId, serverId, username);
        
        const embed = embedBuilder.buildMainMenuEmbed(username);
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
      } catch (error) {
        logger.error(`Error creating character: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: `Failed to begin journey: ${(error as Error).message}`, components: [] });
        } catch (e) {
          logger.warn(`Failed to send error message to Discord (Interaction may be expired). Trace: ${traceId}`);
        }
      }
    } else if (action === 'profile') {
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';
        const username = interaction.user.username;

        const profile = await apiClient.getCharacterProfile(discordId, serverId, username);
        const profileEmbed = embedBuilder.buildProfileEmbed(profile);

        const profileRow = new ActionRowBuilder<ButtonBuilder>()
          .addComponents(
            new ButtonBuilder()
              .setCustomId('cultivate_ascend')
              .setLabel('Ascend (Breakthrough)')
              .setStyle(ButtonStyle.Success)
          );

        await interaction.editReply({ embeds: [profileEmbed], components: [profileRow] });
      } catch (error) {
        logger.error(`Error fetching profile: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: 'An error occurred while fetching your profile. Please try again later.', components: [] });
        } catch (e) {
          logger.warn(`Failed to send error message to Discord (Interaction may be expired). Trace: ${traceId}`);
        }
      }
    } else if (action === 'explore') {
      const embed = embedBuilder.buildLocationSelectEmbed();
      const select = new StringSelectMenuBuilder()
        .setCustomId('cultivate_explore_location')
        .setPlaceholder('Select a location to explore...')
        .addOptions(
          new StringSelectMenuOptionBuilder()
            .setLabel('Novice Village')
            .setDescription('A safe area for beginners. Low Qi density.')
            .setValue('loc_novice'),
          new StringSelectMenuOptionBuilder()
            .setLabel('Azure Cloud Mountain')
            .setDescription('Dangerous peaks with rare beasts. High Qi density.')
            .setValue('loc_azure')
        );

      const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(select);
      await interaction.editReply({ embeds: [embed], components: [row] });
    } else if (action.startsWith('choice_')) {
      const choiceId = action.replace('choice_', '');
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';
        
        // Pass a dummy eventId for MVP, as backend doesn't strictly validate it yet.
        const result = await apiClient.submitExplorationChoice(discordId, serverId, choiceId, 'dummy-event-id');
        const embed = embedBuilder.buildExplorationResultEmbed(result);
        
        await interaction.editReply({ embeds: [embed], components: [] });
      } catch (error) {
        logger.error(`Error resolving choice: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: `Exploration choice failed: ${(error as Error).message}`, components: [] });
        } catch (e) {
          logger.warn(`Failed to send error message to Discord (Interaction may be expired). Trace: ${traceId}`);
        }
      }
    } else if (action === 'ascend') {
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';

        const result = await apiClient.ascend(discordId, serverId);
        const embed = embedBuilder.buildAscendResultEmbed(result);
        
        await interaction.editReply({ embeds: [embed], components: [] });
      } catch (error) {
        logger.error(`Error ascending: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: `Ascension failed: ${(error as Error).message}`, components: [] });
        } catch (e) {
          logger.warn(`Failed to send error message to Discord (Interaction may be expired). Trace: ${traceId}`);
        }
      }
    } else if (action === 'domain') {
      const embed = new EmbedBuilder()
        .setColor('DarkRed')
        .setTitle('Secret Domains')
        .setDescription('Select a secret domain to challenge. Warning: Combat here is perilous!');

      const select = new StringSelectMenuBuilder()
        .setCustomId('cultivate_domain_location')
        .setPlaceholder('Select a secret domain...')
        .addOptions(
          new StringSelectMenuOptionBuilder()
            .setLabel('Goblin Cave (Lv. 1+)')
            .setDescription('A starter domain filled with weak goblins.')
            .setValue('goblin_cave'),
          new StringSelectMenuOptionBuilder()
            .setLabel('Azure Cloud Mountain (Lv. 4+)')
            .setDescription('A dangerous trial on the mountain peaks.')
            .setValue('azure_mountain')
        );

      const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(select);
      await interaction.editReply({ embeds: [embed], components: [row] });
    } else if (action === 'inventory') {
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';
        const username = interaction.user.username;

        const inventory = await apiClient.getInventory(discordId, serverId);
        
        const embed = embedBuilder.buildInventoryEmbed(username, inventory);
        
        const row = new ActionRowBuilder<ButtonBuilder>()
          .addComponents(
            new ButtonBuilder()
              .setCustomId('cultivate_menu')
              .setLabel('Back to Menu')
              .setStyle(ButtonStyle.Secondary)
          );

        await interaction.editReply({ embeds: [embed], components: [row] });
      } catch (error) {
        logger.error(`Error fetching inventory: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: 'Failed to load inventory.', components: [] });
        } catch (e) { }
      }
    } else if (action === 'menu') {
      const username = interaction.user.username;
      const embed = embedBuilder.buildMainMenuEmbed(username);
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
            .setStyle(ButtonStyle.Danger),
          new ButtonBuilder()
            .setCustomId('cultivate_inventory')
            .setLabel('Inventory')
            .setStyle(ButtonStyle.Secondary)
        );
      await interaction.editReply({ embeds: [embed], components: [row] });
    } else {
      await interaction.editReply({ content: `You clicked ${action}. This feature is coming soon!`, components: [] });
    }
  } else if (interaction.isStringSelectMenu()) {
    if (interaction.customId === 'cultivate_explore_location') {
      const locationId = interaction.values[0];
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';

        const event = await apiClient.startExploration(discordId, serverId, locationId);
        const embed = embedBuilder.buildExplorationEventEmbed(event);

        const row = new ActionRowBuilder<ButtonBuilder>();
        event.choices.forEach(c => {
          let style = ButtonStyle.Primary;
          if (c.style === 'DANGER') style = ButtonStyle.Danger;
          if (c.style === 'SUCCESS') style = ButtonStyle.Success;
          if (c.style === 'SECONDARY') style = ButtonStyle.Secondary;

          row.addComponents(
            new ButtonBuilder()
              .setCustomId(`cultivate_choice_${c.choiceId}`)
              .setLabel(c.label)
              .setStyle(style)
          );
        });

        await interaction.editReply({ embeds: [embed], components: [row] });
      } catch (error) {
        logger.error(`Error starting exploration: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: `Exploration failed: ${(error as Error).message}`, components: [] });
        } catch (e) {
          logger.warn(`Failed to send error message to Discord (Interaction may be expired). Trace: ${traceId}`);
        }
      }
    } else if (interaction.customId === 'cultivate_domain_location') {
      const domainId = interaction.values[0];
      try {
        const discordId = interaction.user.id;
        const serverId = interaction.guildId || 'DM';

        const result = await apiClient.enterSecretDomain(discordId, serverId, domainId);
        const embed = embedBuilder.buildSecretDomainResultEmbed(result);

        await interaction.editReply({ embeds: [embed], components: [] });
      } catch (error) {
        logger.error(`Error entering domain: ${error}`, { traceId });
        try {
          await interaction.editReply({ content: `Domain run failed: ${(error as Error).message}`, components: [] });
        } catch (e) {
          logger.warn(`Failed to send error message to Discord (Interaction may be expired). Trace: ${traceId}`);
        }
      }
    }
  }
};
