// Wrapper for .NET Core REST API
import { logger } from '../utils/logger.js';

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

export class CultivationApiClient {
  private readonly baseUrl: string;

  constructor() {
    // Fallback to localhost if not set in .env
    this.baseUrl = process.env.API_BASE_URL || 'http://localhost:5116/api/v1';
  }

  public async getCharacterProfile(discordId: string, serverId: string, username: string): Promise<CharacterProfileDto> {
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

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`API Error: ${response.status} ${response.statusText} - ${errorText}`);
      }

      const data = await response.json();
      return data as CharacterProfileDto;
    } catch (error) {
      logger.error('Failed to fetch character profile', { error });
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

      if (!response.ok) {
        throw new Error(`API Error: ${response.status}`);
      }

      const data = await response.json();
      return data as ServerConfigDto;
    } catch (error) {
      logger.error('Failed to fetch server config', { error });
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
      if (!response.ok && response.status !== 400) {
        throw new Error(`API Error: ${response.status}`);
      }

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
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`API Error: ${response.status} - ${errorText}`);
      }
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
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`API Error: ${response.status} - ${errorText}`);
      }
      return await response.json() as ExplorationResultDto;
    } catch (error) {
      logger.error('Failed to submit exploration choice', { error });
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
