import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

// 1. CORRECCIÓN: Importamos el nombre correcto de la clase
import { DestinosPopularesComponent } from './destinos-populares';

// Importamos los servicios para poder simularlos
import { DestinoService } from '../../proxy/destinos';
import { CalificacionService } from '../../proxy/calificaciones';
import { AuthService } from '@abp/ng.core';
import { ToasterService, ConfirmationService } from '@abp/ng.theme.shared';

describe('DestinosPopularesComponent', () => {
  let component: DestinosPopularesComponent;
  let fixture: ComponentFixture<DestinosPopularesComponent>;

  // 2. Creamos Mocks (simulaciones) de todos los servicios que usa tu constructor
  const mockDestinoService = {
    getPopularDestinations: jasmine.createSpy('getPopularDestinations').and.returnValue(of([])),
    create: jasmine.createSpy('create').and.returnValue(of({}))
  };

  const mockCalificacionService = {
    create: jasmine.createSpy('create').and.returnValue(of({}))
  };

  const mockAuthService = {
    isAuthenticated: true,
    navigateToLogin: jasmine.createSpy('navigateToLogin')
  };

  const mockToasterService = {
    success: jasmine.createSpy('success'),
    error: jasmine.createSpy('error')
  };

  const mockConfirmationService = {
    warn: jasmine.createSpy('warn').and.returnValue(of('confirm'))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DestinosPopularesComponent],
      // 3. Le decimos a Angular que use nuestras simulaciones en lugar de las reales
      providers: [
        { provide: DestinoService, useValue: mockDestinoService },
        { provide: CalificacionService, useValue: mockCalificacionService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: ToasterService, useValue: mockToasterService },
        { provide: ConfirmationService, useValue: mockConfirmationService }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DestinosPopularesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});