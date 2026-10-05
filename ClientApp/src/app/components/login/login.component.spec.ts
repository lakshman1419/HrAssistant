import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { LoginComponent } from './login.component';
import { appRoutes } from '../../app.routes';
import { AUTH_SESSION_STORAGE_KEY } from '../../services/auth.service';

describe('LoginComponent', () => {
  let fixture: ComponentFixture<LoginComponent>;
  let component: LoginComponent;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    sessionStorage.clear();
    await TestBed.configureTestingModule({
      imports: [LoginComponent, HttpClientTestingModule],
      providers: [provideRouter(appRoutes)]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('shows adjacent required errors and does not request login for blank fields', () => {
    component.submit();
    fixture.detectChanges();

    expect(component.loginForm.invalid).toBeTrue();
    expect(fixture.nativeElement.textContent).toContain('Username is required.');
    expect(fixture.nativeElement.textContent).toContain('Password is required.');
    httpMock.expectNone('http://localhost:5118/api/auth/login');
  });

  it('prevents repeat submission while pending and shows a generic credential error', () => {
    component.loginForm.setValue({ username: 'person', password: 'wrong' });
    component.submit();
    component.submit();
    fixture.detectChanges();

    expect(component.isPending).toBeTrue();
    expect(fixture.nativeElement.textContent).toContain('Signing in…');
    const request = httpMock.expectOne('http://localhost:5118/api/auth/login');
    request.flush({ message: 'Specific backend detail' }, { status: 401, statusText: 'Unauthorized' });
    fixture.detectChanges();

    expect(component.isPending).toBeFalse();
    expect(fixture.nativeElement.textContent).toContain('Username or password is incorrect.');
    expect(fixture.nativeElement.textContent).not.toContain('Specific backend detail');
  });

  it('shows a retry message for network or server failures', () => {
    component.loginForm.setValue({ username: 'person', password: 'secret' });
    component.submit();
    httpMock.expectOne('http://localhost:5118/api/auth/login').flush(
      { message: 'internal detail' },
      { status: 500, statusText: 'Server Error' }
    );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('We could not reach the sign-in service. Please try again.');
    expect(fixture.nativeElement.textContent).not.toContain('internal detail');
  });

  it('shows a generic input error for bad requests without exposing backend details', () => {
    component.loginForm.setValue({ username: 'person', password: 'secret' });
    component.submit();
    httpMock.expectOne('http://localhost:5118/api/auth/login').flush(
      { message: 'Specific backend detail' },
      { status: 400, statusText: 'Bad Request' }
    );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Please check your sign-in details and try again.');
    expect(fixture.nativeElement.textContent).not.toContain('Specific backend detail');
    expect(fixture.nativeElement.textContent).not.toContain('We could not reach the sign-in service. Please try again.');
  });

  it('returns to a safe assistant URL with its query parameters after successful login', async () => {
    const router = TestBed.inject(Router);
    await router.navigateByUrl('/login?returnUrl=%2Fassistant%3Ftab%3Dleave');

    component.loginForm.setValue({ username: 'person', password: 'secret' });
    component.submit();
    httpMock.expectOne('http://localhost:5118/api/auth/login').flush({
      user: {
        username: 'person',
        displayName: 'Example Person',
        role: 'Employee'
      }
    });
    await fixture.whenStable();

    expect(router.url).toBe('/assistant?tab=leave');
  });

  it('does not navigate to an external return URL after successful login', async () => {
    const router = TestBed.inject(Router);
    await router.navigateByUrl('/login?returnUrl=https%3A%2F%2Fexample.com%2Fassistant');

    component.loginForm.setValue({ username: 'person', password: 'secret' });
    component.submit();
    httpMock.expectOne('http://localhost:5118/api/auth/login').flush({
      user: {
        username: 'person',
        displayName: 'Example Person',
        role: 'Employee'
      }
    });
    await fixture.whenStable();

    expect(router.url).toBe('/assistant');
  });
});