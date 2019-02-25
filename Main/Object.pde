class Object
{
  int m_objectID;
  String m_name;
  PImage m_texture;
  PVector m_position;
  PVector m_scale;
  float m_angle;
  
  Object(String a_name, String a_fileName)
  {
    Construct(a_name, "Resources/" + a_fileName, new PVector(width/2, height/2), 0, new PVector());
  }
  
  Object(String a_name, String a_fileName, PVector a_position, float a_angle, PVector a_scale)
  {
    Construct(a_name, "Resources/" + a_fileName, a_position, a_angle, a_scale);
  }
  
  Object(Object a_source)
  {
    m_name = a_source.m_name;
    m_texture = a_source.m_texture;
    m_position = a_source.m_position;
    m_scale = a_source.m_scale;
    m_angle = a_source.m_angle;
  }
  
  void Construct(String a_name, String a_fileName, PVector a_position, float a_angle, PVector a_scale)
  {
    m_name = a_name;
    m_texture = loadImage(a_fileName);
    m_position = a_position;
    m_angle = a_angle;
    m_scale = a_scale;    
  }
  
  void Draw()
  {
    pushMatrix();
    rotate(m_angle);
    translate(m_position.x, m_position.y);
    image(m_texture, 0, 0);
    popMatrix();
  }
}
