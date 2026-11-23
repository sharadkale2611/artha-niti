export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  userId: string;
  email: string;
  fullName: string;
  roles: string[];
}