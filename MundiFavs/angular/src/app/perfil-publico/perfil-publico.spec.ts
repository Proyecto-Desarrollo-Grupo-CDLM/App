import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PerfilPublico } from './perfil-publico';

describe('PerfilPublico', () => {
  let component: PerfilPublico;
  let fixture: ComponentFixture<PerfilPublico>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PerfilPublico]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PerfilPublico);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
