export type UserRole = 'Admin' | 'Employee';

export interface AuthUserProfile {
  username: string;
  displayName: string;
  role: UserRole;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  user: AuthUserProfile;
}