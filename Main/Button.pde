class Button
{
  PVector m_position;
  PVector m_radii;
  
  boolean m_clicked;
  
  class Click implements MouseCommand
  {
      public void Execute(float a_dt, MouseInputData a_mouseData)
      {
       if(!a_mouseData.m_pressed)
         m_clicked = true;
      }
  }
}
