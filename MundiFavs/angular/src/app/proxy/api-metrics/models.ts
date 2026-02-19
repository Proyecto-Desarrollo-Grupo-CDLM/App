import type { EntityDto } from '@abp/ng.core';

export interface ApiMetricDto extends EntityDto<string> {
  apiName?: string;
  endpoint?: string;
  durationMs: number;
  statusCode: number;
  isSuccess: boolean;
  executionTime?: string;
}

export interface ApiSummaryDto {
  totalCalls: number;
  errorCount: number;
  averageDurationMs: number;
}
