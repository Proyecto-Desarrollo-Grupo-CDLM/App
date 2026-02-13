import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';

// 1. Importamos tu componente SIN la extensión .ts
import { UserSearchComponent } from './busqueda-usuario'; 
import { PerfilUsuarioService } from '../proxy/usuarios';

describe('UserSearchComponent', () => {
  let component: UserSearchComponent;
  let fixture: ComponentFixture<UserSearchComponent>;

  // 2. Creamos "mocks" (simulaciones) para los servicios que usa tu constructor
  // Esto evita errores de "No provider for PerfilUsuarioService!"
  const mockPerfilUsuarioService = {
    searchUsers: jasmine.createSpy('searchUsers').and.returnValue(of([]))
  };

  const mockRouter = {
    navigate: jasmine.createSpy('navigate')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      // Como es un componente "standalone", va en imports
      imports: [UserSearchComponent],
      // Proveemos los servicios simulados
      providers: [
        { provide: PerfilUsuarioService, useValue: mockPerfilUsuarioService },
        { provide: Router, useValue: mockRouter }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});