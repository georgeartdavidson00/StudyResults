import axios from 'axios';
import { StudyResult, SummaryStats, TimeSeriesData, UploadResponse } from '../types/api';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5173/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const apiService = {
  uploadCsv: async (file: File): Promise<UploadResponse> => {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await api.post<UploadResponse>('/Upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    
    return response.data;
  },

  getStudyResults: async (filters: {
    metricName?: string;
    from?: string;
    to?: string;
    participantId?: string;
  } = {}): Promise<StudyResult[]> => {
    const params = new URLSearchParams();
    
    Object.entries(filters).forEach(([key, value]) => {
      if (value) params.append(key, value);
    });
    
    const response = await api.get<StudyResult[]>(`/StudyResults?${params}`);
    return response.data;
  },

  getSummaryStats: async (metric: string, from?: string, to?: string): Promise<SummaryStats> => {
    const params = new URLSearchParams({ metric });
    if (from) params.append('from', from);
    if (to) params.append('to', to);
    
    const response = await api.get<SummaryStats>(`/Stats?${params}`);
    return response.data;
  },

  getTimeSeriesData: async (metric: string, from?: string, to?: string): Promise<TimeSeriesData[]> => {
    const params = new URLSearchParams({ metric });
    if (from) params.append('from', from);
    if (to) params.append('to', to);
    
    const response = await api.get<TimeSeriesData[]>(`/Stats/timeseries?${params}`);
    return response.data;
  },

  getAvailableMetrics: async (): Promise<string[]> => {
    const response = await api.get<string[]>('/StudyResults/metrics');
    return response.data;
  },

  getParticipantIds: async (): Promise<string[]> => {
    const response = await api.get<string[]>('/StudyResults/participants');
    return response.data;
  },
};
