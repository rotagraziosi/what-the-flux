export interface PeriodSummary {
  id: number;
  startDate: string;
  notes: string | null;
  dayCount: number;
  totalScore: number;
}

export interface PeriodDetail {
  id: number;
  startDate: string;
  notes: string | null;
  dayNumbers: number[];
  rows: DayRow[];
}

export interface DayRow {
  criterionId: number;
  label: string;
  multiplier: number;
  countsByDay: Record<number, number>;
  rowTotal: number;
}

export interface Criterion {
  id: number;
  label: string;
  multiplier: number;
}

export interface CreatePeriodRequest {
  startDate: string;
  notes?: string | null;
  initialDays?: number;
}

export interface CountChangeEvent {
  dayNumber: number;
  criterionId: number;
  count: number;
}
