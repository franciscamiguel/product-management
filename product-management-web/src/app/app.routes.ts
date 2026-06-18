import { Routes } from '@angular/router';

export const routes: Routes = [
	{
		path: '',
		pathMatch: 'full',
		loadComponent: () => import('./features/home/home-page.component').then(m => m.HomePageComponent)
	},
	{
		path: 'produtos',
		loadComponent: () => import('./features/catalog/catalog-page.component').then(m => m.CatalogPageComponent)
	},
	{
		path: '**',
		redirectTo: ''
	}
];
