import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';
import { HrAssistantComponent } from './components/hr-assistant/hr-assistant.component';
import { LoginComponent } from './components/login/login.component';

export const appRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: LoginComponent },
  { path: 'assistant', component: HrAssistantComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'login' }
];