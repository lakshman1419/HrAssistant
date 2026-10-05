import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { HrAssistantComponent } from './components/hr-assistant/hr-assistant.component';
import { AUTH_SESSION_STORAGE_KEY } from './services/auth.service';
import { appRoutes } from './app.routes';
import { provideRouter } from '@angular/router';

describe('application routes', () => {
  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideRouter(appRoutes)]
    });
  });

  afterEach(() => sessionStorage.clear());

  it('redirects the root and unauthenticated assistant visits to login', async () => {
    const harness = await RouterTestingHarness.create();
    const router = TestBed.inject(Router);
    await harness.navigateByUrl('/');
    expect(router.url).toBe('/login');

    await harness.navigateByUrl('/assistant');
    expect(router.url).toBe('/login?returnUrl=%2Fassistant');
  });

  it('allows a session profile to access the existing assistant route', async () => {
    sessionStorage.setItem(AUTH_SESSION_STORAGE_KEY, JSON.stringify({
      username: 'employee',
      displayName: 'Example Employee',
      role: 'Employee'
    }));
    const harness = await RouterTestingHarness.create();

    const assistant = await harness.navigateByUrl('/assistant', HrAssistantComponent);
    expect(assistant.currentUser?.role).toBe('Employee');
  });
});