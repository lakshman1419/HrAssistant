import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AuthService, AUTH_SESSION_STORAGE_KEY } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('posts credentials and stores only the returned safe profile', () => {
    let result: unknown;
    service.login({ username: 'demo-user', password: 'secret-value' }).subscribe(user => result = user);

    const request = httpMock.expectOne('http://localhost:5118/api/auth/login');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ username: 'demo-user', password: 'secret-value' });
    request.flush({ user: { username: 'demo-user', displayName: 'Demo User', role: 'Admin' } });

    expect(result).toEqual({ username: 'demo-user', displayName: 'Demo User', role: 'Admin' });
    expect(sessionStorage.getItem(AUTH_SESSION_STORAGE_KEY)).toBe(JSON.stringify(result));
    expect(sessionStorage.getItem(AUTH_SESSION_STORAGE_KEY)).not.toContain('secret-value');
  });

  it('does not create a session after invalid credentials', () => {
    let receivedError = false;
    service.login({ username: 'demo-user', password: 'wrong' }).subscribe({
      error: () => receivedError = true
    });

    httpMock.expectOne('http://localhost:5118/api/auth/login').flush(
      { message: 'Invalid credentials' },
      { status: 401, statusText: 'Unauthorized' }
    );

    expect(receivedError).toBeTrue();
    expect(sessionStorage.getItem(AUTH_SESSION_STORAGE_KEY)).toBeNull();
  });
});