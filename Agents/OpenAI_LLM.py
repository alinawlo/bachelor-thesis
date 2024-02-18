import sys
import os
from langchain.llms import OpenAI
from langchain.prompts import PromptTemplate
from langchain.chains import LLMChain
from langchain.chat_models import ChatOpenAI



# Set the OpenAI API key
os.environ['OPENAI_API_KEY'] = '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'

# Initialize the OpenAI model
#llm = OpenAI(model_name='text-davinci-003', temperature=0.9, max_tokens=1000)
llm= ChatOpenAI(model_name='gpt-4', temperature=0.9, max_tokens=3000)


# Define the scene template
scene_template ="""
I am developing a Unity scene and require a detailed, step-by-step guide to build a {scene_description} using cubes and spheres as 3D objects. The guide should focus on layout, sizing, and positioning of each element to create a harmonious scene.

When composing the guide, please adhere to these specific instructions:

Start each step with a clear directive (e.g., "Create a grassy ground using a cube").
Give detailed Unity interface actions (e.g., "Right-click in the Hierarchy, select 3D Object > Cube").
Include steps for naming and categorizing each element (e.g., "Name the Cube 'Ground', tag it as 'Terrain'").
Provide exact scale and color values for each object to ensure realism (e.g., "Set Scale to (10, 0.1, 10) for the ground, and color it grass green, RGB: 0, 200, 0").
Emphasize the creation of a ground base as the first step, setting its position and scale to establish scene boundaries.
Clearly state how to avoid overlapping and ensure all elements fit within the ground's boundaries (e.g., "Position objects ensuring they do not cross the ground's edges and there's no overlap").
Include duplication instructions for creating multiple similar objects while maintaining orderly placement (e.g., "Duplicate 'Pathway1' but adjust each copy's position to extend the pathway without overlapping").
Stress on attaching elements together where necessary, and confirm their correct positioning relative to each other and the ground.
Exclude the need for Unity Editor Scripts or complex functionalities.
Format each step like this for clarity:
/*step*/
@"Create the ground:
a. Right-click in the Hierarchy, select 3D Object > Cube.
b. Name the Cube 'Ground', tag it as 'Terrain'.
c. Set its Transform Position to (0, 0, 0), and Scale to (10, 0.1, 10) to define the scene's area.
d. Color it grass green in the Inspector window (RGB: 0, 200, 0)."

Ensure all steps are complete and adhere to this format for a coherent scene layout.
"""

# Create a prompt template
prompt_temp = PromptTemplate(input_variables=["scene_description"], template=scene_template)

# Define the scene description
#description = "a simple scene of a garden using cubes and spheres as 3D objects, with no specific color scheme. The garden should include grass, simple plants, simple trees, a pathway, and lanterns next to the pathway. The scene should be set during daytime."
if len(sys.argv) > 1:
    description = sys.argv[1]
else:
    description = "a simple scene of a garden and a blue ball"

# Format the prompt with the scene description
formatted_prompt = prompt_temp.format(scene_description=description)

# Initialize the language model chain
chain = LLMChain(llm=llm, prompt=prompt_temp)

#  Generate the guide
generated_guide = chain.run(description)

# Print each step
print(generated_guide)