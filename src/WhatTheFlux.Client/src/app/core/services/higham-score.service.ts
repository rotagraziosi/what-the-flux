import { Injectable } from '@angular/core';

export type ScoreCategory = 'normal' | 'heavy' | 'very-heavy';

@Injectable({ providedIn: 'root' })
export class HighamScoreService {
  getCategory(score: number): ScoreCategory {
    if (score < 100) return 'normal';
    if (score <= 300) return 'heavy';
    return 'very-heavy';
  }

  getCategoryLabel(score: number): string {
    const cat = this.getCategory(score);
    if (cat === 'normal') return 'Normal';
    if (cat === 'heavy') return 'Heavy';
    return 'Very Heavy';
  }

  getCategoryColor(score: number): string {
    const cat = this.getCategory(score);
    if (cat === 'normal') return 'var(--score-normal)';
    if (cat === 'heavy') return 'var(--score-heavy)';
    return 'var(--score-very-heavy)';
  }
}
