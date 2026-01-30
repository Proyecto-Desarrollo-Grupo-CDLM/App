import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiPerfilConfig } from './mi-perfil-config';

describe('MiPerfilConfig', () => {
  let component: MiPerfilConfig;
  let fixture: ComponentFixture<MiPerfilConfig>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MiPerfilConfig]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MiPerfilConfig);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
