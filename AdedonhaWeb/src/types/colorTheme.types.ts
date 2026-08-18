export type ColorTheme = 'Blue' | 'Yellow' | 'Red' | 'Green' | 'Purple';

export const COLOR_THEME_LABELS: Record<ColorTheme, string> = {
  Blue: 'Azul e Marinho',
  Yellow: 'Amarelo e Preto',
  Red: 'Vermelho e Rosé',
  Green: 'Verde e Esmeralda',
  Purple: 'Roxo e Ciano',
};

export const COLOR_THEME_STORAGE_KEY = 'adedonha.colorTheme';

const VALID_COLOR_THEMES: ColorTheme[] = ['Blue', 'Yellow', 'Red', 'Green', 'Purple'];

export function isColorTheme(value: string | null): value is ColorTheme {
  return value !== null && (VALID_COLOR_THEMES as string[]).includes(value);
}
