// Wrapper for .NET Core REST API
import { logger } from '../utils/logger.js';

export class ApiBusinessError extends Error {
  constructor(message: string, public statusCode: number) {
    super(message);
    this.name = 'ApiBusinessError';
  }
}

export interface CharacterProfileDto {
  discordId: string;
  serverId: string;
  username: string;
  level: number;
  realmName: string;
  currentQi: number;
  dailyQiLimit: number;
  spiritStones: number;
  currentState: string;
  targetQi: number;
  requiredBreakthroughItemId?: string;
  requiredBreakthroughItemQuantity: number;
  baseStats: {
    strength: number;
    agility: number;
    luck: number;
    health: number;
    mana: number;
  };
}

export interface ServerConfigDto {
  serverId: string;
  chatToEarnChannels: string[];
  isActive: boolean;
}

export interface GainQiResultDto {
  success: boolean;
  message: string;
  currentQi: number;
  gainedQi: number;
}

export interface AscendResultDto {
  success: boolean;
  message: string;
  oldLevel: number;
  newLevel: number;
  newBaseStats?: any;
}

export interface RewardItemDto {
  itemId: string;
  quantity: number;
  itemType: string;
}

export interface SecretDomainResultDto {
  success: boolean;
  message: string;
  isVictory: boolean;
  battleLog: string[];
  rewardSpiritStones: number;
  rewardItems: RewardItemDto[];
}

export interface InventoryItemDto {
  itemId: string;
  quantity: number;
  itemType: string;
}

export interface EquippedGearDto {
  head?: string;
  chest?: string;
  weapon?: string;
  artifact?: string;
}

export interface InventoryDto {
  id: string;
  characterId: string;
  items: InventoryItemDto[];
  equippedGear: EquippedGearDto;
}

export class CultivationApiClient {
  private readonly baseUrl: string;

  private async handleResponse(response: Response): Promise<void> {
    if (!response.ok) {
      const errorText = await response.text();
      try {
        if (response.status === 400) {
          const problem = JSON.parse(errorText);
          if (problem && problem.title === 'Domain Rule Violation' && problem.detail) {
            throw new ApiBusinessError(problem.detail, response.status);
          }
        }
      } catch (e) {
        if (e instanceof ApiBusinessError) throw e;
      }
      throw new Error(`API Error: ${response.status} - ${errorText}`);
    }
  }

  constructor() {
    // Fallback to localhost if not set in .env
    this.baseUrl = process.env.API_BASE_URL || 'http://localhost:5116/api/v1';
  }

  public async getCharacterProfile(discordId: string, serverId: string, username: string): Promise<CharacterProfileDto | null> {
    const url = new URL(`${this.baseUrl}/characters/profile`);
    url.searchParams.append('discordId', discordId);
    url.searchParams.append('serverId', serverId);
    url.searchParams.append('username', username);

    logger.info(`Fetching character profile for discordId: ${discordId}`, { url: url.toString() });

    try {
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
      });

      if (response.status === 404) {
        return null;
      }

      await this.handleResponse(response);

      const data = await response.json();
      return data as CharacterProfileDto;
    } catch (error) {
      logger.error('Failed to fetch character profile', { error });
      throw error;
    }
  }

  public async createCharacter(discordId: string, serverId: string, username: string): Promise<CharacterProfileDto> {
    const url = new URL(`${this.baseUrl}/characters/create`);
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({ discordId, serverId, username })
      });

      await this.handleResponse(response);

      return await response.json() as CharacterProfileDto;
    } catch (error) {
      logger.error('Failed to create character', { error });
      throw error;
    }
  }

  public async getServerConfig(serverId: string): Promise<ServerConfigDto> {
    const url = new URL(`${this.baseUrl}/servers/${serverId}/config`);
    
    try {
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
      });

      await this.handleResponse(response);

      const data = await response.json();
      return data as ServerConfigDto;
    } catch (error) {
      logger.error('Failed to fetch server config', { error });
      throw error;
    }
  }

  public async getInventory(discordId: string, serverId: string): Promise<InventoryDto> {
    const url = new URL(`${this.baseUrl}/inventory`);
    url.searchParams.append('discordId', discordId);
    url.searchParams.append('serverId', serverId);
    try {
      const response = await fetch(url.toString(), {
        headers: { 'Accept': 'application/json' },
      });
      await this.handleResponse(response);
      return await response.json() as InventoryDto;
    } catch (error) {
      logger.error('Failed to get inventory', { error });
      throw error;
    }
  }

  public async equipItem(discordId: string, serverId: string, itemId: string, slot: string): Promise<boolean> {
    const url = new URL(`${this.baseUrl}/inventory/equip`);
    url.searchParams.append('discordId', discordId);
    url.searchParams.append('serverId', serverId);
    url.searchParams.append('itemId', itemId);
    url.searchParams.append('slot', slot);
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: { 'Accept': 'application/json' },
      });
      return response.ok;
    } catch (error) {
      logger.error('Failed to equip item', { error });
      throw error;
    }
  }

  public async gainQi(discordId: string, serverId: string, username: string): Promise<GainQiResultDto> {
    const url = new URL(`${this.baseUrl}/characters/gain-qi`);
    
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: JSON.stringify({ discordId, serverId, username })
      });

      // We expect 400 or 200 for business logic results (cooldowns vs success)
      await this.handleResponse(response);

      const data = await response.json();
      return data as GainQiResultDto;
    } catch (error) {
      logger.error('Failed to gain Qi', { error });
      throw error;
    }
  }

  public async startExploration(discordId: string, serverId: string, locationId: string): Promise<ExplorationEventDto> {
    const url = new URL(`${this.baseUrl}/activities/explore`);
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({ discordId, serverId, locationId })
      });
      await this.handleResponse(response);
      return await response.json() as ExplorationEventDto;
    } catch (error) {
      logger.error('Failed to start exploration', { error });
      throw error;
    }
  }

  public async submitExplorationChoice(discordId: string, serverId: string, choiceId: string, eventId: string = ""): Promise<ExplorationResultDto> {
    const url = new URL(`${this.baseUrl}/activities/explore/choice`);
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({ discordId, serverId, choiceId, eventId })
      });
      await this.handleResponse(response);
      return await response.json() as ExplorationResultDto;
    } catch (error) {
      logger.error('Failed to submit exploration choice', { error });
      throw error;
    }
  }

  public async ascend(discordId: string, serverId: string): Promise<AscendResultDto> {
    const url = new URL(`${this.baseUrl}/characters/ascend`);
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({ discordId, serverId })
      });

      await this.handleResponse(response);

      return await response.json() as AscendResultDto;
    } catch (error) {
      logger.error('Failed to ascend', { error });
      throw error;
    }
  }

  public async enterSecretDomain(discordId: string, serverId: string, domainId: string): Promise<SecretDomainResultDto> {
    const url = new URL(`${this.baseUrl}/activities/secret-domain`);
    try {
      const response = await fetch(url.toString(), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({ discordId, serverId, domainId })
      });

      await this.handleResponse(response);

      return await response.json() as SecretDomainResultDto;
    } catch (error) {
      logger.error('Failed to enter secret domain', { error });
      throw error;
    }
  }
}

export interface ExplorationChoiceDto {
  choiceId: string;
  label: string;
  style: string;
}

export interface ExplorationEventDto {
  eventId: string;
  eventType: string;
  title: string;
  description: string;
  imageUrl?: string;
  choices: ExplorationChoiceDto[];
}

export interface ExplorationResultDto {
  success: boolean;
  title: string;
  narrative: string;
  qiReward: number;
  spiritStonesReward: number;
}

export const apiClient = new CultivationApiClient();
