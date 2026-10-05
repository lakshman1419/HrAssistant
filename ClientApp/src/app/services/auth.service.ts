import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AuthUserProfile, LoginRequest, LoginResponse } from '../models/auth.model';

export const AUTH_SESSION_STORAGE_KEY = 'hr-assistant-profile';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5118/api/auth/login';

  get currentUser(): AuthUserProfile | null {
    try {
      const serializedProfile = sessionStorage.getItem(AUTH_SESSION_STORAGE_KEY);
      if (!serializedProfile) {
        return null;
      }

      const profile: unknown = JSON.parse(serializedProfile);
      if (!isAuthUserProfile(profile)) {
        sessionStorage.removeItem(AUTH_SESSION_STORAGE_KEY);
        return null;
      }

      return profile;
    } catch {
      return null;
    }
  }

  login(credentials: LoginRequest): Observable<AuthUserProfile> {
    return this.http.post<LoginResponse>(this.apiUrl, credentials).pipe(
      map(({ user }) => {
        if (!isAuthUserProfile(user)) {
          throw new Error('The login response did not contain a valid user profile.');
        }

        const profile: AuthUserProfile = {
          username: user.username,
          displayName: user.displayName,
          role: user.role
        };
        sessionStorage.setItem(AUTH_SESSION_STORAGE_KEY, JSON.stringify(profile));
        return profile;
      })
    );
  }

  logout(): void {
    sessionStorage.removeItem(AUTH_SESSION_STORAGE_KEY);
  }
}

function isAuthUserProfile(value: unknown): value is AuthUserProfile {
  if (typeof value !== 'object' || value === null) {
    return false;
  }

  const profile = value as Record<string, unknown>;
  return typeof profile['username'] === 'string'
    && typeof profile['displayName'] === 'string'
    && (profile['role'] === 'Admin' || profile['role'] === 'Employee');
}