import { jwtDecode } from 'jwt-decode';

const NAME_IDENTIFIER_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

export const ADMIN_ROLE = 'Admin';

export interface AuthUser {
  id: string;
  email: string;
  name: string;
  roles: string[];
  isAdmin: boolean;
}

interface RawTokenClaims {
  [claim: string]: unknown;
  sub?: string;
  email?: string;
  name?: string;
  role?: string | string[];
  exp: number;
}

function toStringArray(value: unknown): string[] {
  if (value == null) return [];
  return Array.isArray(value) ? value.map(String) : [String(value)];
}

export function decodeAuthUser(token: string): AuthUser {
  const claims = jwtDecode<RawTokenClaims>(token);
  const id = (claims.sub ?? claims[NAME_IDENTIFIER_CLAIM]) as string;
  const email = (claims.email ?? claims[EMAIL_CLAIM]) as string;
  const name = (claims.name ?? claims[NAME_CLAIM]) as string;
  const roles = toStringArray(claims.role ?? claims[ROLE_CLAIM]);
  return { id, email, name, roles, isAdmin: roles.includes(ADMIN_ROLE) };
}
