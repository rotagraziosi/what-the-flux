import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'periods', pathMatch: 'full' },
  {
    path: 'periods',
    loadComponent: () =>
      import('./features/period-list/period-list.component').then(
        (m) => m.PeriodListComponent
      ),
  },
  {
    path: 'periods/new',
    loadComponent: () =>
      import('./features/new-period/new-period.component').then(
        (m) => m.NewPeriodComponent
      ),
  },
  {
    path: 'periods/:id',
    loadComponent: () =>
      import('./features/period-detail/period-detail.component').then(
        (m) => m.PeriodDetailComponent
      ),
    data: { renderMode: 'client' },
  },
];
